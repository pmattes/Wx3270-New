#!/usr/bin/env python3
#
# Copyright (c) 2021-2022 Paul Mattes.
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#     * Redistributions of source code must retain the above copyright
#       notice, this list of conditions and the following disclaimer.
#     * Redistributions in binary form must reproduce the above copyright
#       notice, this list of conditions and the following disclaimer in the
#       documentation and/or other materials provided with the distribution.
#     * Neither the names of Paul Mattes nor the names of his contributors
#       may be used to endorse or promote products derived from this software
#       without specific prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY PAUL MATTES "AS IS" AND ANY EXPRESS OR IMPLIED
# WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
# MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
# EVENT SHALL PAUL MATTES BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
# PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
# OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
# WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
# OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
# ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#
# wx3270 smoke tests

import filecmp
import os
from subprocess import Popen, PIPE, DEVNULL
import unittest
import re
import requests
import tempfile
import time

import Common.Test.playback as playback
import Common.Test.cti as cti

class TestWx3270Smoke(cti.cti):

    # x3270 smoke test
    def test_wx3270_smoke(self):

        # Start 'playback' to feed wx3270.
        pport, pts = cti.unused_port()
        wport, wts = cti.unused_port()
        with playback.playback(self, 'Test/ibmlink.trc', port=pport) as p:
            pts.close()

            # Create a profile.
            with open('Test/TestProfile.wx3270', encoding='utf-8') as f:
                content = f.read()
            sub = re.sub('%PPORT%', str(pport), re.sub('%WPORT%', str(wport), content))
            handle, profile_name = tempfile.mkstemp(suffix='.wx3270')
            os.close(handle)
            with open(profile_name, 'w', encoding='utf-8') as f:
                print(sub, end='', file=f)

            # Start wx3270.
            wx3270 = Popen(['wx3270/bin/x64/Debug/wx3270.exe', '-nowatch', '-readonly', '-profile', profile_name])
            self.check_listen(wport)
            wts.close()
            os.unlink(profile_name)

            # Feed wx3270 some data.
            p.send_records(4)

            # Give it a moment to compose itself.
            # time.sleep(1)

            # Dump the window contents.
            (handle, name) = tempfile.mkstemp(suffix='.gif')
            os.close(handle)
            r = requests.get(f'http://127.0.0.1:{wport}/3270/rest/json/uSnapScreen("{name}")')
            # print('file is', name)
            self.assertEqual(requests.codes.ok, r.status_code)

        # Wait for the processes to exit.
        wx3270.kill()
        wx3270.wait(timeout=2)

        # Make sure the image is correct.
        if not filecmp.cmp(name, 'Test/ibmlink.gif'):
            os.system(f'start name')
        self.assertTrue(filecmp.cmp(name, 'Test/ibmlink.gif'))
        os.unlink(name)

if __name__ == '__main__':
    unittest.main()
